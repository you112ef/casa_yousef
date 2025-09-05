import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import { LogOut, Languages, User2 } from "lucide-react";
import { useAuth } from "../auth/AuthProvider";
import { Link } from "react-router-dom";

export default function Topbar() {
  const { t, i18n } = useTranslation();
  const { logout, user } = useAuth();
  const toggleLang = () => {
    const next = i18n.language === "ar" ? "en" : "ar";
    i18n.changeLanguage(next);
    localStorage.setItem("lang", next);
    document.documentElement.dir = next === "ar" ? "rtl" : "ltr";
  };
  return (
    <header className="sticky top-0 z-40 w-full border-b bg-teal-600 text-white">
      <div className="mx-auto flex h-14 max-w-7xl items-center justify-between gap-4 px-4">
        <div className="flex items-center gap-3">
          <div className="h-8 w-8 rounded bg-white/20" />
          <span className="text-lg font-semibold">Sky CASA</span>
        </div>
        <nav className="hidden items-center gap-4 md:flex">
          <Link className="text-sm opacity-90 hover:opacity-100" to="/">{t("dashboard")}</Link>
          <Link className="text-sm opacity-90 hover:opacity-100" to="/data">{t("dataView")}</Link>
          <Link className="text-sm opacity-90 hover:opacity-100" to="/analysis">{t("analysis")}</Link>
          <Link className="text-sm opacity-90 hover:opacity-100" to="/ai">{t("ai")}</Link>
          <Link className="text-sm opacity-90 hover:opacity-100" to="/who-report">{t("whoReport")}</Link>
          <Link className="text-sm opacity-90 hover:opacity-100" to="/settings">{t("settings")}</Link>
        </nav>
        <div className="flex items-center gap-2">
          <Button variant="secondary" size="sm" onClick={toggleLang} className="bg-white/20 text-white hover:bg-white/30">
            <Languages className="mr-1 h-4 w-4" /> {i18n.language.toUpperCase()}
          </Button>
          <div className="hidden items-center gap-2 sm:flex">
            <User2 className="h-5 w-5" />
            <span className="text-sm opacity-90">{user?.name || user?.email || ""}</span>
          </div>
          <Button size="sm" variant="secondary" onClick={logout} className="bg-white/20 text-white hover:bg-white/30">
            <LogOut className="mr-1 h-4 w-4" /> {t("logout")}
          </Button>
        </div>
      </div>
    </header>
  );
}
