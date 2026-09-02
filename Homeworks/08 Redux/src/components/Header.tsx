import React from 'react';
import { AppBar, Toolbar, Typography, Button, Box } from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { logout } from '../store/slices/authSlice';


export const Header: React.FC = () => {
    const dispatch = useAppDispatch();
    const { user, isAuthenticated } = useAppSelector((state) => state.auth);
    return (
      <AppBar position="static">
        <Toolbar>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, flexGrow: 1 }}>
            <Typography variant="h6" component="div">
              Список задач
            </Typography>
            <Button color="inherit" component={RouterLink} to="/">
              Главная
            </Button>
          </Box>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
            {isAuthenticated && user ? (
              <>
                <Typography>
                  {user.lastName} {user.firstName}
                </Typography>
                <Button color="inherit" onClick={() => dispatch(logout())}>
                  Выйти
                </Button>
              </>
            ) : (
              <>
                <Button color="inherit" component={RouterLink} to="/login">
                  Войти
                </Button>
                <Button color="inherit" component={RouterLink} to="/register">
                  Зарегистрироваться
                </Button>
              </>
            )}
          </Box>
        </Toolbar>
      </AppBar>
    );
  };
