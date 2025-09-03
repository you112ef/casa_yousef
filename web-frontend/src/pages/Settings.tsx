import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

export default function Settings() {
  return (
    <div className="space-y-6 p-4">
      <section className="rounded border p-4">
        <h3 className="mb-4 text-lg font-semibold">Lab Information</h3>
        <div className="grid grid-cols-1 gap-3 md:grid-cols-2">
          <Input placeholder="Lab Name" />
          <Input placeholder="Address" />
          <Input placeholder="Phone" />
          <Input placeholder="Email" />
        </div>
        <div className="mt-4"><Button className="bg-teal-600 hover:bg-teal-700">Save</Button></div>
      </section>
      <section className="rounded border p-4">
        <h3 className="mb-4 text-lg font-semibold">AI Settings</h3>
        <div className="grid grid-cols-1 gap-3 md:grid-cols-3">
          <Input placeholder="FPS" />
          <Input placeholder="µm-per-pixel" />
          <Input placeholder="Duration (s)" />
        </div>
        <div className="mt-4"><Button className="bg-teal-600 hover:bg-teal-700">Save</Button></div>
      </section>
    </div>
  );
}
