import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

export default function StatCard({ label, value, hint, color = "bg-teal-600" }: { label: string; value: string | number; hint?: string; color?: string; }) {
  return (
    <Card className="overflow-hidden">
      <div className={`${color} h-1 w-full`} />
      <CardHeader>
        <CardTitle className="text-sm text-muted-foreground">{label}</CardTitle>
      </CardHeader>
      <CardContent>
        <div className="text-3xl font-bold">{value}</div>
        {hint && <div className="mt-1 text-xs text-muted-foreground">{hint}</div>}
      </CardContent>
    </Card>
  );
}
