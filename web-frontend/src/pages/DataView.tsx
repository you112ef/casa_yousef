import { useEffect, useState } from "react";
import DataTable from "../components/DataTable";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Button } from "@/components/ui/button";
import { getPatients, getCBC } from "../api/endpoints";

export default function DataView() {
  const [table, setTable] = useState("patients");
  const [rows, setRows] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);

  const load = async () => {
    setLoading(true);
    try {
      if (table === "patients") {
        const d = await getPatients({ limit: 50 });
        setRows(d.patients);
      } else if (table === "cbc") {
        const d = await getCBC({ limit: 50 });
        setRows(d.results);
      } else {
        setRows([]);
      }
    } finally { setLoading(false); }
  };

  useEffect(() => { load(); }, [table]);

  return (
    <div className="space-y-4 p-4">
      <div className="flex flex-wrap items-center gap-2">
        <Select value={table} onValueChange={setTable}>
          <SelectTrigger className="w-56"><SelectValue placeholder="Table" /></SelectTrigger>
          <SelectContent>
            <SelectItem value="patients">patients</SelectItem>
            <SelectItem value="cbc">cbc</SelectItem>
            <SelectItem value="urine">urine</SelectItem>
            <SelectItem value="stool">stool</SelectItem>
            <SelectItem value="kidney">kidney</SelectItem>
            <SelectItem value="liver">liver</SelectItem>
          </SelectContent>
        </Select>
        <Button onClick={load} disabled={loading} className="bg-teal-600 hover:bg-teal-700">{loading ? "..." : "Reload"}</Button>
      </div>
      <DataTable data={rows} />
    </div>
  );
}
