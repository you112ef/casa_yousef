import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";

export default function DataTable<T extends Record<string, any>>({ data }: { data: T[] }) {
  const headers = data.length ? Object.keys(data[0]) : [];
  return (
    <div className="w-full overflow-auto rounded border">
      <Table>
        <TableHeader>
          <TableRow>
            {headers.map((h) => (
              <TableHead key={h} className="whitespace-nowrap">{h}</TableHead>
            ))}
          </TableRow>
        </TableHeader>
        <TableBody>
          {data.map((row, i) => (
            <TableRow key={i}>
              {headers.map((h) => (
                <TableCell key={h} className="whitespace-nowrap">{String(row[h] ?? "").slice(0, 200)}</TableCell>
              ))}
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
}
