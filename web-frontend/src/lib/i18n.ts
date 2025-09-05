import i18n from "i18next";
import { initReactI18next } from "react-i18next";

const resources = {
  ar: {
    translation: {
      login: "تسجيل الدخول",
      username: "اسم المستخدم",
      password: "كلمة المرور",
      signIn: "دخول",
      dashboard: "لوحة التحكم",
      dataView: "عرض البيانات",
      analysis: "التحاليل",
      ai: "الذكاء الاصطناعي",
      whoReport: "تقرير WHO",
      settings: "الإعدادات",
      logout: "تسجيل الخروج",
      recentAnalyses: "التحاليل الأخيرة",
      quickActions: "إجراءات سريعة",
      uploadImage: "رفع صورة",
      uploadVideo: "رفع فيديو",
      analyze: "تحليل",
      duration: "المدة (ث)"
    },
  },
  en: {
    translation: {
      login: "Login",
      username: "Username",
      password: "Password",
      signIn: "Sign In",
      dashboard: "Dashboard",
      dataView: "Data View",
      analysis: "Analysis",
      ai: "AI",
      whoReport: "WHO Report",
      settings: "Settings",
      logout: "Logout",
      recentAnalyses: "Recent Analyses",
      quickActions: "Quick Actions",
      uploadImage: "Upload Image",
      uploadVideo: "Upload Video",
      analyze: "Analyze",
      duration: "Duration (s)"
    },
  },
};

i18n.use(initReactI18next).init({
  resources,
  lng: localStorage.getItem("lang") || "ar",
  fallbackLng: "en",
  interpolation: { escapeValue: false },
});

document.documentElement.dir = (i18n.language || "ar") === "ar" ? "rtl" : "ltr";

export default i18n;
