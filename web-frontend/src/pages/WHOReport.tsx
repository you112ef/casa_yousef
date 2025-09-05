import { Button } from "@/components/ui/button";

export default function WHOReport() {
  const print = () => window.print();
  return (
    <div className="space-y-4 p-4">
      <div className="rounded border">
        <div className="bg-teal-600 p-4 text-white">
          <div className="text-xl font-semibold">WHO Report</div>
          <div className="opacity-90">Patient: — | Date: —</div>
        </div>
        <div className="space-y-4 p-4">
          <section>
            <h3 className="mb-2 font-semibold">Basic Parameters</h3>
            <div className="grid grid-cols-2 gap-2 md:grid-cols-3">
              {Array.from({ length: 9 }).map((_, i) => (
                <div key={i} className="rounded border bg-muted/30 p-2">—</div>
              ))}
            </div>
          </section>
          <section>
            <h3 className="mb-2 font-semibold">CASA</h3>
            <div className="grid grid-cols-2 gap-2 md:grid-cols-3">
              {Array.from({ length: 6 }).map((_, i) => (
                <div key={i} className="rounded border bg-muted/30 p-2">—</div>
              ))}
            </div>
          </section>
        </div>
      </div>
      <div className="flex gap-2 print:hidden">
        <Button className="bg-teal-600 hover:bg-teal-700" onClick={print}>Print</Button>
        <Button variant="outline">Export</Button>
      </div>
    </div>
  );
}
