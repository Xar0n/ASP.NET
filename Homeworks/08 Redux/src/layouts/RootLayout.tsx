import React from 'react';
import { Outlet } from 'react-router-dom';
import { Container, CssBaseline } from '@mui/material';
import { Header } from '../components/Header';

export const RootLayout: React.FC = () => {
  return (
    <>
      <CssBaseline />
      <Header />
      <Container sx={{ mt: 4 }}>
        <Outlet />
      </Container>
    </>
  );
};
