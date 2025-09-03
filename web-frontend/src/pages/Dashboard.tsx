import { useEffect, useState } from "react";
import StatCard from "../components/StatCard";
import { useTranslation } from "react-i18next";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

export default function Dashboard() {
  const { t } = useTranslation();
  const [stats, setStats] = useState([
    { label: "Patients", value: "-" },
    { label: "Analyses", value: "-" },
    { label: "AI Runs", value: "-" },
    { label: "Reports", value: "-" },
    { label: "CBC", value: "-" },
    { label: "Urine", value: "-" },
    { label: "Stool", value: "-" },
    { label: "Users", value: "-" },
  ]);

  useEffect(() => {
    setStats((s) => s.map((x) => ({ ...x, value: x.value === "-" ? "—" : x.value })));
  }, []);

  return (
    <div className="space-y-6 p-4">
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {stats.map((s, i) => (
          <StatCard key={i} label={s.label} value={s.value} />
        ))}
      </div>

      <div className="grid grid-cols-1 gap-4 lg:grid-cols-3">
        <Card className="lg:col-span-1">
          <CardHeader><CardTitle>{t("quickActions")}</CardTitle></CardHeader>
          <CardContent>
            <div className="grid grid-cols-2 gap-2">
              {[
                "مريض جديد", "CBC", "بول", "براز", "كِلى", "كبد", "AI", "عرض البيانات", "الإعدادات", "اختبار النظام",
              ].map((x) => (
                <button key={x} className="rounded bg-teal-600 px-3 py-2 text-sm text-white hover:bg-teal-700">{x}</button>
              ))}
            </div>
          </CardContent>
        </Card>
        <Card className="lg:col-span-2">
          <CardHeader><CardTitle>{t("recentAnalyses")}</CardTitle></CardHeader>
          <CardContent>
            <div className="h-64 rounded border bg-muted/30" />
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
