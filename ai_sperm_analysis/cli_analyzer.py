#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Sky CASA - Command Line Interface for C# Integration
واجهة سطر الأوامر للتكامل مع C#

Usage:
python cli_analyzer.py --type image --media "path/to/image.jpg" --patient 1
python cli_analyzer.py --type video --media "path/to/video.mp4" --patient 1 --duration 15
"""

import argparse
import sys
import os
import json
from analyze_media import SpermAnalyzer

def main():
    """
    واجهة سطر الأوامر لتحليل الحيوانات المنوية
    """
    parser = argparse.ArgumentParser(description='Sky CASA - AI Sperm Analysis CLI')
    
    parser.add_argument('--type', choices=['image', 'video'], required=True,
                       help='نوع التحليل: image أو video')
    parser.add_argument('--media', required=True,
                       help='مسار الملف (صورة أو فيديو)')
    parser.add_argument('--patient', type=int, required=True,
                       help='معرف المريض')
    parser.add_argument('--duration', type=int, default=15,
                       help='مدة تحليل الفيديو بالثواني (افتراضي: 15)')
    parser.add_argument('--output', default='console',
                       help='نوع الإخراج: console أو json')
    
    args = parser.parse_args()
    
    try:
        # التحقق من وجود الملف
        if not os.path.exists(args.media):
            raise FileNotFoundError(f"الملف غير موجود: {args.media}")
        
        # تهيئة المحلل
        analyzer = SpermAnalyzer()
        
        # تنفيذ التحليل
        if args.type == 'image':
            results = analyzer.analyze_image(args.media, args.patient, save_results=True)
        elif args.type == 'video':
            results = analyzer.analyze_video(args.media, args.patient, args.duration, save_results=True)
        
        # تحضير النتائج للإخراج
        output_results = format_results_for_csharp(results)
        
        if args.output == 'json':
            # إخراج JSON للتكامل مع C#
            print(json.dumps(output_results, ensure_ascii=False, indent=2))
        else:
            # إخراج نصي للوحة التحكم
            print_console_results(output_results)
        
        return 0  # نجاح
        
    except Exception as e:
        error_result = {
            'success': False,
            'error': str(e),
            'timestamp': results.get('timestamp', '') if 'results' in locals() else ''
        }
        
        if args.output == 'json':
            print(json.dumps(error_result, ensure_ascii=False))
        else:
            print(f"خطأ: {e}", file=sys.stderr)
        
        return 1  # خطأ

def format_results_for_csharp(results):
    """
    تنسيق النتائج للتكامل مع C#
    
    Args:
        results: نتائج التحليل من Python
        
    Returns:
        dict: نتائج منسقة لـ C#
    """
    formatted = {
        'success': True,
        'patientId': results.get('patient_id'),
        'analysisType': results.get('analysis_type'),
        'timestamp': results.get('timestamp'),
        'totalCount': results.get('total_count', 0),
        'aiConfidence': results.get('ai_confidence', 0.0),
        'concentrationEstimation': results.get('concentration_estimation', 0.0),
        'whoCompliance': results.get('who_compliance', False),
        'originalImagePath': results.get('image_path', ''),
        'originalVideoPath': results.get('video_path', ''),
        'analyzedImagePath': results.get('analyzed_image_path', ''),
        'analyzedVideoPath': results.get('analyzed_video_path', ''),
        'heatmapPath': results.get('heatmap_path', ''),
    }
    
    # إضافة بيانات الفيديو إذا كانت متوفرة
    if results.get('analysis_type') == 'video':
        formatted.update({
            'durationSeconds': results.get('duration_seconds', 0),
            'totalFrames': results.get('total_frames', 0),
            'totalTracks': results.get('total_tracks', 0),
            'validTracks': results.get('valid_tracks', 0)
        })
        
        # معايير CASA
        casa_metrics = results.get('casa_metrics', {})
        if casa_metrics:
            formatted['casaMetrics'] = {
                'vclMean': casa_metrics.get('vcl_mean', 0.0),
                'vslMean': casa_metrics.get('vsl_mean', 0.0),
                'vapMean': casa_metrics.get('vap_mean', 0.0),
                'linMean': casa_metrics.get('lin_mean', 0.0),
                'strMean': casa_metrics.get('str_mean', 0.0),
                'wobMean': casa_metrics.get('wob_mean', 0.0),
                'alhMean': casa_metrics.get('alh_mean', 0.0),
                'bcfMean': casa_metrics.get('bcf_mean', 0.0)
            }
        
        # تحليل الحركة
        motility = results.get('motility_analysis', {})
        if motility:
            formatted['motilityAnalysis'] = {
                'rapidProgressivePercent': motility.get('rapid_progressive_percent', 0.0),
                'slowProgressivePercent': motility.get('slow_progressive_percent', 0.0),
                'nonProgressivePercent': motility.get('non_progressive_percent', 0.0),
                'immotilePercent': motility.get('immotile_percent', 0.0),
                'totalProgressivePercent': motility.get('total_progressive_percent', 0.0),
                'totalMotilePercent': motility.get('total_motile_percent', 0.0)
            }
    
    return formatted

def print_console_results(results):
    """
    طباعة النتائج في وحدة التحكم
    
    Args:
        results: النتائج المنسقة
    """
    print("🧬 Sky CASA - نتائج التحليل بالذكاء الاصطناعي")
    print("=" * 50)
    
    print(f"📊 نوع التحليل: {results['analysisType']}")
    print(f"👤 معرف المريض: {results['patientId']}")
    print(f"⏰ وقت التحليل: {results['timestamp']}")
    print(f"🎯 معامل الثقة: {results['aiConfidence']:.1%}")
    print()
    
    if results['analysisType'] == 'image':
        print("📸 نتائج تحليل الصورة:")
        print(f"   • العدد الكلي: {results['totalCount']} حيوان منوي")
        print(f"   • التركيز المقدر: {results['concentrationEstimation']:.1f} مليون/مل")
        print(f"   • متوافق مع WHO: {'نعم' if results['whoCompliance'] else 'لا'}")
        
        if results.get('analyzedImagePath'):
            print(f"   • الصورة المحللة: {results['analyzedImagePath']}")
        if results.get('heatmapPath'):
            print(f"   • الخريطة الحرارية: {results['heatmapPath']}")
    
    elif results['analysisType'] == 'video':
        print("🎬 نتائج تحليل الفيديو:")
        print(f"   • المدة: {results['durationSeconds']} ثانية")
        print(f"   • الإطارات: {results['totalFrames']}")
        print(f"   • المسارات: {results['totalTracks']} (صالحة: {results['validTracks']})")
        
        if 'casaMetrics' in results:
            casa = results['casaMetrics']
            print("   🧪 معايير CASA:")
            print(f"      - VCL: {casa['vclMean']:.1f} μm/s")
            print(f"      - VSL: {casa['vslMean']:.1f} μm/s")
            print(f"      - LIN: {casa['linMean']:.1f}%")
        
        if 'motilityAnalysis' in results:
            motility = results['motilityAnalysis']
            print("   🏃 تحليل الحركة:")
            print(f"      - سريع ومتقدم: {motility['rapidProgressivePercent']:.1f}%")
            print(f"      - بطيء ومتقدم: {motility['slowProgressivePercent']:.1f}%")
            print(f"      - حركة في المكان: {motility['nonProgressivePercent']:.1f}%")
            print(f"      - غير متحرك: {motility['immotilePercent']:.1f}%")
        
        if results.get('analyzedVideoPath'):
            print(f"   • الفيديو المحلل: {results['analyzedVideoPath']}")
    
    print()
    print("✅ تم إنهاء التحليل بنجاح")

if __name__ == "__main__":
    exit_code = main()
    sys.exit(exit_code)