import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

const items = [
  { key: "cbc", title: "CBC", desc: "Complete Blood Count" },
  { key: "urine", title: "Urinalysis", desc: "Urinary analysis" },
  { key: "stool", title: "Stool", desc: "Stool analysis" },
  { key: "kidney", title: "Kidney", desc: "Kidney function" },
  { key: "liver", title: "Liver", desc: "Liver function" },
  { key: "ai", title: "AI", desc: "Sperm Analysis (AI)" },
];

export default function Analysis() {
  return (
    <div className="p-4">
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {items.map((x) => (
          <a key={x.key} href={x.key === "ai" ? "/ai" : `#/${x.key}`}>
            <Card className="hover:shadow-md">
              <div className="h-1 w-full bg-teal-600" />
              <CardHeader><CardTitle>{x.title}</CardTitle></CardHeader>
              <CardContent><div className="text-sm text-muted-foreground">{x.desc}</div></CardContent>
            </Card>
          </a>
        ))}
      </div>
    </div>
  );
}
