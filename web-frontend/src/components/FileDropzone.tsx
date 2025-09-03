import { useRef, useState } from "react";
import { Button } from "@/components/ui/button";

export default function FileDropzone({ accept, onFile }: { accept?: string; onFile: (file: File) => void }) {
  const [hover, setHover] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);
  return (
    <div
      onDragOver={(e) => { e.preventDefault(); setHover(true); }}
      onDragLeave={() => setHover(false)}
      onDrop={(e) => { e.preventDefault(); setHover(false); const f = e.dataTransfer.files?.[0]; if (f) onFile(f); }}
      className={`flex h-40 cursor-pointer items-center justify-center rounded border-2 border-dashed ${hover ? "border-teal-600 bg-teal-50" : "border-border"}`}
      onClick={() => inputRef.current?.click()}
    >
      <input ref={inputRef} type="file" accept={accept} hidden onChange={(e) => { const f = e.target.files?.[0]; if (f) onFile(f); }} />
      <div className="text-sm text-muted-foreground">Drop file here or click to select</div>
    </div>
  );
}
