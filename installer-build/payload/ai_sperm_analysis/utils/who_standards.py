#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Sky CASA - WHO Standards Checker
فاحص معايير منظمة الصحة العالمية للحيوانات المنوية

WHO Laboratory Manual 6th Edition (2021) Reference Values
"""

class WHOStandards:
    def __init__(self):
        """
        معايير WHO الحديثة للحيوانات المنوية (الطبعة السادسة 2021)
        """
        
        # المعايير الأساسية - القيم المرجعية السفلية (5th percentile)
        self.reference_values = {
            # الحجم والتركيز
            'volume_ml': 1.4,                    # الحجم (مل)
            'concentration_million_ml': 16,      # التركيز (مليون/مل)
            'total_count_million': 39,           # العدد الكلي (مليون)
            
            # الحركة
            'total_motility_percent': 42,        # الحركة الكلية (%)
            'progressive_motility_percent': 30,  # الحركة التقدمية (%)
            
            # الشكل والحيوية
            'normal_morphology_percent': 4,      # الشكل الطبيعي (%)
            'viability_percent': 54,             # الحيوية (%)
            
            # CASA Parameters
            'vcl_um_s': 50,                      # VCL (μm/s)
            'vsl_um_s': 25,                      # VSL (μm/s)
            'vap_um_s': 35,                      # VAP (μm/s)
            'lin_percent': 50,                   # LIN (%)
            'str_percent': 80,                   # STR (%)
            'wob_percent': 70,                   # WOB (%)
            'alh_um': 2.5,                       # ALH (μm)
            'bcf_hz': 10,                        # BCF (Hz)
            
            # الخصائص الفيزيائية
            'ph_min': 7.2,                       # pH أدنى
            'ph_max': 8.0,                       # pH أعلى
            'liquefaction_time_min': 60,         # وقت الإسالة (دقيقة)
            
            # الخلايا الأخرى
            'wbc_million_ml': 1.0,               # خلايا الدم البيضاء (حد أعلى)
        }
        
        # معايير الجودة للحركة
        self.motility_grades = {
            'A': {'vsl_min': 25, 'lin_min': 50, 'description': 'سريع ومتقدم'},
            'B': {'vsl_min': 5, 'vsl_max': 25, 'lin_min': 25, 'description': 'بطيء ومتقدم'},
            'C': {'vsl_min': 0, 'vsl_max': 5, 'description': 'حركة في المكان'},
            'D': {'vsl': 0, 'description': 'غير متحرك'}
        }
        
        # تصنيفات التشخيص
        self.diagnostic_categories = {
            'normozoospermia': 'طبيعي',
            'oligozoospermia': 'قلة العدد',
            'asthenozoospermia': 'ضعف الحركة', 
            'teratozoospermia': 'تشوه الشكل',
            'oligoasthenozoospermia': 'قلة العدد وضعف الحركة',
            'oligoteratozoospermia': 'قلة العدد وتشوه الشكل',
            'asthenoteratozoospermia': 'ضعف الحركة وتشوه الشكل',
            'oligoasthenoteratozoospermia': 'قلة العدد وضعف الحركة وتشوه الشكل',
            'azoospermia': 'انعدام الحيوانات المنوية',
            'severe_oligozoospermia': 'قلة شديدة في العدد'
        }
    
    def check_volume_compliance(self, volume):
        """
        فحص امتثال الحجم لمعايير WHO
        
        Args:
            volume: حجم السائل المنوي بالمل
            
        Returns:
            dict: نتائج الفحص
        """
        is_normal = volume >= self.reference_values['volume_ml']
        
        return {
            'parameter': 'Volume',
            'value': volume,
            'reference': f"≥ {self.reference_values['volume_ml']} mL",
            'is_normal': is_normal,
            'interpretation': 'طبيعي' if is_normal else 'أقل من الطبيعي',
            'clinical_significance': 'قد يشير إلى مشاكل في الغدد التناسلية' if not is_normal else 'طبيعي'
        }
    
    def check_concentration_compliance(self, concentration):
        """
        فحص امتثال التركيز لمعايير WHO
        
        Args:
            concentration: تركيز الحيوانات المنوية (مليون/مل)
            
        Returns:
            dict: نتائج الفحص
        """
        is_normal = concentration >= self.reference_values['concentration_million_ml']
        
        severity = 'طبيعي'
        if concentration < 1:
            severity = 'شديد جداً'
        elif concentration < 5:
            severity = 'شديد'
        elif concentration < 15:
            severity = 'متوسط'
        elif concentration < self.reference_values['concentration_million_ml']:
            severity = 'خفيف'
        
        return {
            'parameter': 'Concentration',
            'value': concentration,
            'reference': f"≥ {self.reference_values['concentration_million_ml']} million/mL",
            'is_normal': is_normal,
            'interpretation': 'طبيعي' if is_normal else 'قلة في التركيز',
            'severity': severity,
            'clinical_significance': self.get_oligozoospermia_significance(concentration)
        }
    
    def check_total_count_compliance(self, total_count):
        """
        فحص امتثال العدد الكلي لمعايير WHO
        
        Args:
            total_count: العدد الكلي للحيوانات المنوية (مليون)
            
        Returns:
            dict: نتائج الفحص
        """
        is_normal = total_count >= self.reference_values['total_count_million']
        
        return {
            'parameter': 'Total Count',
            'value': total_count,
            'reference': f"≥ {self.reference_values['total_count_million']} million",
            'is_normal': is_normal,
            'interpretation': 'طبيعي' if is_normal else 'قلة في العدد الكلي',
            'clinical_significance': 'قد يؤثر على القدرة الإنجابية' if not is_normal else 'طبيعي'
        }
    
    def check_motility_compliance(self, total_motility, progressive_motility):
        """
        فحص امتثال الحركة لمعايير WHO
        
        Args:
            total_motility: الحركة الكلية (%)
            progressive_motility: الحركة التقدمية (%)
            
        Returns:
            dict: نتائج الفحص
        """
        total_normal = total_motility >= self.reference_values['total_motility_percent']
        progressive_normal = progressive_motility >= self.reference_values['progressive_motility_percent']
        
        return {
            'parameter': 'Motility',
            'total_motility': {
                'value': total_motility,
                'reference': f"≥ {self.reference_values['total_motility_percent']}%",
                'is_normal': total_normal
            },
            'progressive_motility': {
                'value': progressive_motility,
                'reference': f"≥ {self.reference_values['progressive_motility_percent']}%",
                'is_normal': progressive_normal
            },
            'overall_normal': total_normal and progressive_normal,
            'interpretation': self.get_motility_interpretation(total_motility, progressive_motility),
            'clinical_significance': self.get_asthenozoospermia_significance(total_motility, progressive_motility)
        }
    
    def check_morphology_compliance(self, normal_morphology):
        """
        فحص امتثال الشكل لمعايير WHO
        
        Args:
            normal_morphology: نسبة الشكل الطبيعي (%)
            
        Returns:
            dict: نتائج الفحص
        """
        is_normal = normal_morphology >= self.reference_values['normal_morphology_percent']
        
        return {
            'parameter': 'Morphology',
            'value': normal_morphology,
            'reference': f"≥ {self.reference_values['normal_morphology_percent']}%",
            'is_normal': is_normal,
            'interpretation': 'طبيعي' if is_normal else 'تشوه في الشكل',
            'clinical_significance': self.get_teratozoospermia_significance(normal_morphology)
        }
    
    def check_casa_compliance(self, casa_metrics):
        """
        فحص امتثال معايير CASA لمعايير WHO
        
        Args:
            casa_metrics: dict معايير CASA
            
        Returns:
            dict: نتائج شاملة للفحص
        """
        results = {}
        
        # فحص كل معيار CASA
        casa_parameters = ['vcl_um_s', 'vsl_um_s', 'vap_um_s', 'lin_percent', 'str_percent', 'wob_percent']
        
        for param in casa_parameters:
            if param in casa_metrics:
                value = casa_metrics[param]
                reference = self.reference_values[param]
                is_normal = value >= reference
                
                results[param] = {
                    'value': value,
                    'reference': f"≥ {reference}",
                    'is_normal': is_normal,
                    'interpretation': 'طبيعي' if is_normal else 'أقل من الطبيعي'
                }
        
        # تقييم شامل
        normal_count = sum(1 for r in results.values() if r['is_normal'])
        total_count = len(results)
        
        results['overall_assessment'] = {
            'normal_parameters': normal_count,
            'total_parameters': total_count,
            'compliance_percentage': (normal_count / total_count * 100) if total_count > 0 else 0,
            'overall_interpretation': self.get_casa_overall_interpretation(normal_count, total_count)
        }
        
        return results
    
    def check_full_compliance(self, analysis_data):
        """
        فحص شامل لجميع المعايير
        
        Args:
            analysis_data: dict جميع بيانات التحليل
            
        Returns:
            dict: تقييم شامل مع التشخيص
        """
        results = {
            'individual_assessments': {},
            'diagnostic_category': '',
            'fertility_potential': '',
            'recommendations': []
        }
        
        # فحص المعايير الأساسية
        if 'volume_ml' in analysis_data:
            results['individual_assessments']['volume'] = self.check_volume_compliance(analysis_data['volume_ml'])
        
        if 'concentration_million_ml' in analysis_data:
            results['individual_assessments']['concentration'] = self.check_concentration_compliance(analysis_data['concentration_million_ml'])
        
        if 'total_motility_percent' in analysis_data and 'progressive_motility_percent' in analysis_data:
            results['individual_assessments']['motility'] = self.check_motility_compliance(
                analysis_data['total_motility_percent'], 
                analysis_data['progressive_motility_percent']
            )
        
        if 'normal_morphology_percent' in analysis_data:
            results['individual_assessments']['morphology'] = self.check_morphology_compliance(analysis_data['normal_morphology_percent'])
        
        # فحص معايير CASA
        if 'casa_metrics' in analysis_data:
            results['individual_assessments']['casa'] = self.check_casa_compliance(analysis_data['casa_metrics'])
        
        # تحديد التشخيص
        results['diagnostic_category'] = self.determine_diagnostic_category(analysis_data)
        results['fertility_potential'] = self.assess_fertility_potential(analysis_data)
        results['recommendations'] = self.generate_recommendations(results['individual_assessments'])
        
        return results
    
    def determine_diagnostic_category(self, data):
        """
        تحديد فئة التشخيص حسب معايير WHO
        
        Args:
            data: بيانات التحليل
            
        Returns:
            str: فئة التشخيص
        """
        concentration = data.get('concentration_million_ml', 0)
        total_motility = data.get('total_motility_percent', 0)
        morphology = data.get('normal_morphology_percent', 0)
        
        # تحديد المشاكل
        oligo = concentration < self.reference_values['concentration_million_ml']
        astheno = total_motility < self.reference_values['total_motility_percent']
        terato = morphology < self.reference_values['normal_morphology_percent']
        
        # تحديد التشخيص
        if concentration == 0:
            return self.diagnostic_categories['azoospermia']
        elif concentration < 5:
            return self.diagnostic_categories['severe_oligozoospermia']
        elif oligo and astheno and terato:
            return self.diagnostic_categories['oligoasthenoteratozoospermia']
        elif oligo and astheno:
            return self.diagnostic_categories['oligoasthenozoospermia']
        elif oligo and terato:
            return self.diagnostic_categories['oligoteratozoospermia']
        elif astheno and terato:
            return self.diagnostic_categories['asthenoteratozoospermia']
        elif oligo:
            return self.diagnostic_categories['oligozoospermia']
        elif astheno:
            return self.diagnostic_categories['asthenozoospermia']
        elif terato:
            return self.diagnostic_categories['teratozoospermia']
        else:
            return self.diagnostic_categories['normozoospermia']
    
    def assess_fertility_potential(self, data):
        """
        تقييم إمكانية الإنجاب
        
        Args:
            data: بيانات التحليل
            
        Returns:
            str: تقييم إمكانية الإنجاب
        """
        concentration = data.get('concentration_million_ml', 0)
        progressive_motility = data.get('progressive_motility_percent', 0)
        morphology = data.get('normal_morphology_percent', 0)
        
        # حساب نقاط الخصوبة
        fertility_score = 0
        
        # نقاط التركيز
        if concentration >= 20:
            fertility_score += 30
        elif concentration >= 15:
            fertility_score += 25
        elif concentration >= 10:
            fertility_score += 20
        elif concentration >= 5:
            fertility_score += 10
        
        # نقاط الحركة
        if progressive_motility >= 40:
            fertility_score += 30
        elif progressive_motility >= 32:
            fertility_score += 25
        elif progressive_motility >= 25:
            fertility_score += 20
        elif progressive_motility >= 15:
            fertility_score += 10
        
        # نقاط الشكل
        if morphology >= 6:
            fertility_score += 25
        elif morphology >= 4:
            fertility_score += 20
        elif morphology >= 2:
            fertility_score += 15
        elif morphology >= 1:
            fertility_score += 10
        
        # تحديد إمكانية الإنجاب
        if fertility_score >= 75:
            return 'ممتاز - إمكانية إنجاب عالية'
        elif fertility_score >= 60:
            return 'جيد - إمكانية إنجاب جيدة'
        elif fertility_score >= 45:
            return 'متوسط - إمكانية إنجاب متوسطة'
        elif fertility_score >= 30:
            return 'منخفض - إمكانية إنجاب منخفضة'
        else:
            return 'ضعيف جداً - إمكانية إنجاب ضعيفة'
    
    def generate_recommendations(self, assessments):
        """
        إنشاء توصيات طبية
        
        Args:
            assessments: نتائج التقييمات
            
        Returns:
            list: قائمة التوصيات
        """
        recommendations = []
        
        # توصيات عامة
        recommendations.append('إعادة الفحص خلال 2-3 أشهر للتأكد من النتائج')
        recommendations.append('الحفاظ على نمط حياة صحي')
        
        # توصيات خاصة حسب النتائج
        if 'concentration' in assessments and not assessments['concentration']['is_normal']:
            recommendations.append('استشارة طبيب الذكورة لتقييم أسباب قلة التركيز')
            recommendations.append('فحص الهرمونات (FSH, LH, التستوستيرون)')
        
        if 'motility' in assessments and not assessments['motility']['overall_normal']:
            recommendations.append('فحص عوامل مضادة للأكسدة')
            recommendations.append('تجنب التدخين والكحول')
        
        if 'morphology' in assessments and not assessments['morphology']['is_normal']:
            recommendations.append('فحص العوامل الوراثية')
            recommendations.append('تجنب التعرض للمواد الكيميائية والحرارة العالية')
        
        return recommendations
    
    # Helper methods for interpretations
    def get_oligozoospermia_significance(self, concentration):
        if concentration >= self.reference_values['concentration_million_ml']:
            return 'تركيز طبيعي'
        elif concentration >= 10:
            return 'انخفاض طفيف - قد يحتاج متابعة'
        elif concentration >= 5:
            return 'انخفاض متوسط - يحتاج تقييم طبي'
        else:
            return 'انخفاض شديد - يحتاج تدخل طبي عاجل'
    
    def get_asthenozoospermia_significance(self, total_motility, progressive_motility):
        if (total_motility >= self.reference_values['total_motility_percent'] and 
            progressive_motility >= self.reference_values['progressive_motility_percent']):
            return 'حركة طبيعية'
        else:
            return 'ضعف في الحركة - قد يؤثر على القدرة الإنجابية'
    
    def get_teratozoospermia_significance(self, morphology):
        if morphology >= self.reference_values['normal_morphology_percent']:
            return 'شكل طبيعي'
        else:
            return 'تشوه في الشكل - قد يؤثر على الإخصاب'
    
    def get_motility_interpretation(self, total_motility, progressive_motility):
        if (total_motility >= self.reference_values['total_motility_percent'] and 
            progressive_motility >= self.reference_values['progressive_motility_percent']):
            return 'حركة طبيعية'
        elif total_motility < self.reference_values['total_motility_percent']:
            return 'انخفاض في الحركة الكلية'
        else:
            return 'انخفاض في الحركة التقدمية'
    
    def get_casa_overall_interpretation(self, normal_count, total_count):
        percentage = (normal_count / total_count * 100) if total_count > 0 else 0
        
        if percentage >= 80:
            return 'معايير CASA ممتازة'
        elif percentage >= 60:
            return 'معايير CASA جيدة'
        elif percentage >= 40:
            return 'معايير CASA مقبولة'
        else:
            return 'معايير CASA تحتاج تحسين'

# مثال للاستخدام
if __name__ == "__main__":
    print("🏥 WHO Standards Checker Test")
    print("=" * 40)
    
    # إنشاء فاحص معايير WHO
    who_checker = WHOStandards()
    
    # بيانات تجريبية
    sample_data = {
        'volume_ml': 3.0,
        'concentration_million_ml': 45,
        'total_count_million': 135,
        'total_motility_percent': 60,
        'progressive_motility_percent': 45,
        'normal_morphology_percent': 8,
        'casa_metrics': {
            'vcl_um_s': 65,
            'vsl_um_s': 32,
            'lin_percent': 55,
            'str_percent': 85
        }
    }
    
    # فحص شامل
    results = who_checker.check_full_compliance(sample_data)
    
    print("نتائج الفحص الشامل:")
    print(f"التشخيص: {results['diagnostic_category']}")
    print(f"إمكانية الإنجاب: {results['fertility_potential']}")
    print("\nالتوصيات:")
    for rec in results['recommendations']:
        print(f"- {rec}")