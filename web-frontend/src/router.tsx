import { createBrowserRouter, Outlet } from "react-router-dom";
import RequireAuth from "./auth/RequireAuth";
import MainLayout from "./layouts/MainLayout";
import Login from "./pages/Login";
import Dashboard from "./pages/Dashboard";
import DataView from "./pages/DataView";
import Analysis from "./pages/Analysis";
import SpermAI from "./pages/SpermAI";
import WHOReport from "./pages/WHOReport";
import Settings from "./pages/Settings";
import NotFound from "./pages/NotFound";

function Shell() {
  return (
    <RequireAuth>
      <MainLayout>
        <Outlet />
      </MainLayout>
    </RequireAuth>
  );
}

export const router = createBrowserRouter([
  { path: "/login", element: <Login /> },
  {
    path: "/",
    element: <Shell />,
    children: [
      { index: true, element: <Dashboard /> },
      { path: "data", element: <DataView /> },
      { path: "analysis", element: <Analysis /> },
      { path: "ai", element: <SpermAI /> },
      { path: "who-report", element: <WHOReport /> },
      { path: "settings", element: <Settings /> },
      { path: "*", element: <NotFound /> },
    ],
  },
]);
