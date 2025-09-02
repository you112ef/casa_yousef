#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Sky CASA - CASA Metrics Calculator
حاسب معايير CASA للحيوانات المنوية

CASA Parameters:
- VCL: Curvilinear Velocity (السرعة المنحنية)
- VSL: Straight-line Velocity (السرعة المستقيمة)  
- VAP: Average Path Velocity (متوسط سرعة المسار)
- LIN: Linearity (الخطية)
- STR: Straightness (الاستقامة)
- WOB: Wobble (التذبذب)
- ALH: Amplitude of Lateral Head displacement (سعة الانحراف الجانبي)
- BCF: Beat Cross Frequency (تردد عبور النبضة)
"""

import numpy as np
from scipy.spatial.distance import euclidean
from scipy.signal import find_peaks
import math

class CASACalculator:
    def __init__(self, pixel_to_micron=0.5, fps=30):
        """
        تهيئة حاسب CASA
        
        Args:
            pixel_to_micron: نسبة تحويل البكسل إلى ميكرون
            fps: عدد الإطارات في الثانية
        """
        self.pixel_to_micron = pixel_to_micron
        self.fps = fps
    
    def calculate_velocities(self, positions, timestamps):
        """
        حساب السرعات المختلفة
        
        Args:
            positions: list of (x, y) positions
            timestamps: list of timestamps in milliseconds
            
        Returns:
            tuple: (VCL, VSL, VAP) in μm/s
        """
        if len(positions) < 2:
            return 0, 0, 0
        
        # تحويل المواضع إلى ميكرونات
        positions_um = [(x * self.pixel_to_micron, y * self.pixel_to_micron) 
                       for x, y in positions]
        
        # تحويل الأوقات إلى ثوان
        times_s = [(t / 1000.0) for t in timestamps]
        
        # حساب VCL - السرعة المنحنية (مجموع المسافات بين النقاط المتتالية)
        vcl_distances = []
        for i in range(1, len(positions_um)):
            dist = euclidean(positions_um[i-1], positions_um[i])
            vcl_distances.append(dist)
        
        total_time = times_s[-1] - times_s[0]
        if total_time <= 0:
            return 0, 0, 0
        
        vcl = sum(vcl_distances) / total_time
        
        # حساب VSL - السرعة المستقيمة (المسافة المباشرة من البداية للنهاية)
        straight_distance = euclidean(positions_um[0], positions_um[-1])
        vsl = straight_distance / total_time
        
        # حساب VAP - متوسط سرعة المسار (المسار المنعم)
        smoothed_path = self.smooth_path(positions_um)
        vap_distances = []
        for i in range(1, len(smoothed_path)):
            dist = euclidean(smoothed_path[i-1], smoothed_path[i])
            vap_distances.append(dist)
        
        vap = sum(vap_distances) / total_time if vap_distances else vsl
        
        return vcl, vsl, vap
    
    def calculate_linearity_params(self, vcl, vsl, vap):
        """
        حساب معاملات الخطية
        
        Args:
            vcl, vsl, vap: السرعات المختلفة
            
        Returns:
            tuple: (LIN, STR, WOB) as percentages
        """
        # LIN - الخطية = VSL/VCL * 100
        lin = (vsl / vcl * 100) if vcl > 0 else 0
        
        # STR - الاستقامة = VSL/VAP * 100
        str_val = (vsl / vap * 100) if vap > 0 else 0
        
        # WOB - التذبذب = VAP/VCL * 100
        wob = (vap / vcl * 100) if vcl > 0 else 0
        
        return lin, str_val, wob
    
    def calculate_amplitude_lateral_head(self, positions):
        """
        حساب سعة الانحراف الجانبي للرأس (ALH)
        
        Args:
            positions: list of (x, y) positions
            
        Returns:
            float: ALH in micrometers
        """
        if len(positions) < 3:
            return 0
        
        positions_um = [(x * self.pixel_to_micron, y * self.pixel_to_micron) 
                       for x, y in positions]
        
        # حساب الخط المستقيم من البداية للنهاية
        start, end = positions_um[0], positions_um[-1]
        
        # حساب المسافات العمودية من كل نقطة إلى الخط المستقيم
        perpendicular_distances = []
        
        for pos in positions_um[1:-1]:  # تجاهل نقطتي البداية والنهاية
            distance = self.point_to_line_distance(pos, start, end)
            perpendicular_distances.append(distance)
        
        # ALH هو متوسط المسافات العمودية
        alh = np.mean(perpendicular_distances) if perpendicular_distances else 0
        
        return alh
    
    def calculate_beat_frequency(self, positions, timestamps):
        """
        حساب تردد عبور النبضة (BCF)
        
        Args:
            positions: list of (x, y) positions
            timestamps: list of timestamps
            
        Returns:
            float: BCF in Hz
        """
        if len(positions) < 10:
            return 0
        
        positions_um = [(x * self.pixel_to_micron, y * self.pixel_to_micron) 
                       for x, y in positions]
        
        # حساب الخط الرئيسي للحركة
        start, end = positions_um[0], positions_um[-1]
        
        # حساب الانحرافات العمودية عن الخط الرئيسي
        deviations = []
        for pos in positions_um:
            deviation = self.point_to_line_distance(pos, start, end)
            deviations.append(deviation)
        
        # البحث عن القمم في الانحرافات (تقاطعات مع الخط الرئيسي)
        deviations = np.array(deviations)
        peaks, _ = find_peaks(deviations, height=np.std(deviations) * 0.5)
        
        # حساب التردد
        total_time = (timestamps[-1] - timestamps[0]) / 1000.0  # بالثواني
        if total_time > 0 and len(peaks) > 1:
            bcf = len(peaks) / total_time
        else:
            bcf = 0
        
        return bcf
    
    def smooth_path(self, positions):
        """
        تنعيم المسار باستخدام moving average
        
        Args:
            positions: list of (x, y) positions
            
        Returns:
            list: smoothed positions
        """
        if len(positions) < 3:
            return positions
        
        window_size = min(5, len(positions))
        smoothed = []
        
        for i in range(len(positions)):
            start_idx = max(0, i - window_size // 2)
            end_idx = min(len(positions), i + window_size // 2 + 1)
            
            x_vals = [pos[0] for pos in positions[start_idx:end_idx]]
            y_vals = [pos[1] for pos in positions[start_idx:end_idx]]
            
            smooth_x = np.mean(x_vals)
            smooth_y = np.mean(y_vals)
            
            smoothed.append((smooth_x, smooth_y))
        
        return smoothed
    
    def point_to_line_distance(self, point, line_start, line_end):
        """
        حساب المسافة العمودية من نقطة إلى خط مستقيم
        
        Args:
            point: (x, y) النقطة
            line_start: (x, y) بداية الخط
            line_end: (x, y) نهاية الخط
            
        Returns:
            float: المسافة العمودية
        """
        x0, y0 = point
        x1, y1 = line_start
        x2, y2 = line_end
        
        # إذا كانت نقطتا الخط متطابقتين
        if x1 == x2 and y1 == y2:
            return euclidean(point, line_start)
        
        # حساب المسافة العمودية باستخدام الصيغة الرياضية
        numerator = abs((y2-y1)*x0 - (x2-x1)*y0 + x2*y1 - y2*x1)
        denominator = math.sqrt((y2-y1)**2 + (x2-x1)**2)
        
        return numerator / denominator
    
    def analyze_motility_pattern(self, positions, timestamps):
        """
        تحليل نمط الحركة بشكل شامل
        
        Args:
            positions: list of (x, y) positions
            timestamps: list of timestamps
            
        Returns:
            dict: تحليل شامل لنمط الحركة
        """
        if len(positions) < 5:
            return {'pattern': 'insufficient_data'}
        
        # حساب السرعات
        vcl, vsl, vap = self.calculate_velocities(positions, timestamps)
        lin, str_val, wob = self.calculate_linearity_params(vcl, vsl, vap)
        alh = self.calculate_amplitude_lateral_head(positions)
        bcf = self.calculate_beat_frequency(positions, timestamps)
        
        # تصنيف نوع الحركة
        pattern_type = self.classify_movement_pattern(vcl, vsl, lin, str_val, alh)
        
        # تقييم جودة الحركة
        quality_score = self.calculate_movement_quality(vcl, vsl, lin, str_val)
        
        return {
            'vcl': vcl,
            'vsl': vsl,
            'vap': vap,
            'lin': lin,
            'str': str_val,
            'wob': wob,
            'alh': alh,
            'bcf': bcf,
            'pattern': pattern_type,
            'quality_score': quality_score,
            'who_grade': self.get_who_grade(vsl, lin)
        }
    
    def classify_movement_pattern(self, vcl, vsl, lin, str_val, alh):
        """
        تصنيف نمط الحركة
        
        Returns:
            str: نوع النمط
        """
        if vcl < 5:
            return 'immotile'
        elif vsl >= 25 and lin >= 50:
            return 'rapid_progressive'
        elif vsl >= 5 and lin >= 25:
            return 'slow_progressive'
        elif vcl >= 5 and alh > 2:
            return 'hyperactivated'
        else:
            return 'non_progressive'
    
    def calculate_movement_quality(self, vcl, vsl, lin, str_val):
        """
        حساب نقاط جودة الحركة (0-100)
        
        Returns:
            float: نقاط الجودة
        """
        # معايير الجودة المختلفة
        velocity_score = min(vcl / 100 * 30, 30)  # 30% للسرعة
        linearity_score = min(lin / 100 * 25, 25)  # 25% للخطية
        progression_score = min(vsl / 50 * 25, 25)  # 25% للتقدم
        straightness_score = min(str_val / 100 * 20, 20)  # 20% للاستقامة
        
        total_score = velocity_score + linearity_score + progression_score + straightness_score
        
        return min(total_score, 100)
    
    def get_who_grade(self, vsl, lin):
        """
        تصنيف حسب معايير WHO
        
        Returns:
            str: التصنيف (A, B, C, D)
        """
        if vsl >= 25 and lin >= 50:
            return 'A'  # Rapid progressive
        elif vsl >= 5 and lin >= 25:
            return 'B'  # Slow progressive
        elif vsl > 0:
            return 'C'  # Non-progressive
        else:
            return 'D'  # Immotile

# مثال للاستخدام والاختبار
if __name__ == "__main__":
    print("📐 CASA Metrics Calculator Test")
    print("=" * 40)
    
    # إنشاء حاسب CASA
    casa = CASACalculator(pixel_to_micron=0.5, fps=30)
    
    # بيانات تجريبية لمسار حيوان منوي
    test_positions = [
        (100, 100), (102, 101), (105, 102), (108, 104), (112, 105),
        (115, 107), (119, 108), (122, 110), (126, 111), (130, 113)
    ]
    test_timestamps = [0, 33, 66, 100, 133, 166, 200, 233, 266, 300]  # milliseconds
    
    # تحليل المسار
    analysis = casa.analyze_motility_pattern(test_positions, test_timestamps)
    
    print("نتائج التحليل:")
    print(f"VCL: {analysis['vcl']:.2f} μm/s")
    print(f"VSL: {analysis['vsl']:.2f} μm/s")
    print(f"VAP: {analysis['vap']:.2f} μm/s")
    print(f"LIN: {analysis['lin']:.2f}%")
    print(f"STR: {analysis['str']:.2f}%")
    print(f"WOB: {analysis['wob']:.2f}%")
    print(f"ALH: {analysis['alh']:.2f} μm")
    print(f"BCF: {analysis['bcf']:.2f} Hz")
    print(f"Pattern: {analysis['pattern']}")
    print(f"Quality Score: {analysis['quality_score']:.1f}/100")
    print(f"WHO Grade: {analysis['who_grade']}")