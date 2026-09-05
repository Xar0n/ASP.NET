import { createBrowserRouter, Navigate, Outlet } from 'react-router-dom';
import { RootLayout } from './layouts/RootLayout';
import { HomePage } from './pages/HomePage';
import { Login } from './pages/LoginPage';
import { Register } from './pages/RegisterPage';
import { NotFound } from './pages/NotFoundPage';
import { useAppSelector } from './store/hooks';


const ProtectedRoute = ({  redirectPath = '/login' }: { redirectPath?: string }) => {
  const { isAuthenticated } = useAppSelector((state) => state.auth);
  if (!isAuthenticated) {
    return <Navigate to={redirectPath} replace />;
  }
  return <Outlet />;
};

export const router = createBrowserRouter([
  {
    path: '/',
    element: <RootLayout />,
    children: [
      {
        element: <ProtectedRoute  />,
          children: [
            {
              path: '/', element: <HomePage />
            }
          ]
      },
      { path: 'login', element: <Login /> },
      { path: 'register', element: <Register /> },
      { path: '*', element: <NotFound /> },
    ],
  },
]);