import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import FileDropzone from "../components/FileDropzone";
import { analyzeImage, analyzeVideo } from "../api/endpoints";
import { Input } from "@/components/ui/input";

export default function SpermAI() {
  const { t } = useTranslation();
  const [image, setImage] = useState<File | null>(null);
  const [video, setVideo] = useState<File | null>(null);
  const [duration, setDuration] = useState<number>(10);
  const [result, setResult] = useState<any>(null);
  const [loading, setLoading] = useState(false);

  const runImage = async () => {
    if (!image) return;
    setLoading(true);
    try { setResult(await analyzeImage(image)); } finally { setLoading(false); }
  };
  const runVideo = async () => {
    if (!video) return;
    setLoading(true);
    try { setResult(await analyzeVideo(video, duration)); } finally { setLoading(false); }
  };

  return (
    <div className="space-y-6 p-4">
      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
        <div className="space-y-2">
          <div className="text-sm font-medium">{t("uploadImage")}</div>
          <FileDropzone accept="image/*" onFile={setImage} />
          <Button className="bg-teal-600 hover:bg-teal-700" onClick={runImage} disabled={!image || loading}>{t("analyze")}</Button>
        </div>
        <div className="space-y-2">
          <div className="text-sm font-medium">{t("uploadVideo")}</div>
          <FileDropzone accept="video/*" onFile={setVideo} />
          <div className="flex items-center gap-2">
            <label className="text-sm">{t("duration")}</label>
            <Input type="number" className="w-28" value={duration} onChange={(e) => setDuration(Number(e.target.value || 0))} />
            <Button className="bg-teal-600 hover:bg-teal-700" onClick={runVideo} disabled={!video || loading}>{t("analyze")}</Button>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 gap-4 lg:grid-cols-3">
        <div className="h-64 rounded border bg-muted/30" />
        <div className="h-64 rounded border bg-muted/30" />
        <div className="h-64 rounded border bg-muted/30" />
      </div>

      <div className="rounded border p-3 text-sm">{result ? <pre className="overflow-auto">{JSON.stringify(result, null, 2)}</pre> : ""}</div>
    </div>
  );
}
