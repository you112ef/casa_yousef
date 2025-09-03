import Topbar from "../components/Topbar";

export default function MainLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="min-h-screen">
      <Topbar />
      <main className="mx-auto max-w-7xl">
        {children}
      </main>
    </div>
  );
}
