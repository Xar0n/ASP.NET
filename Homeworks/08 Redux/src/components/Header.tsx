import React from 'react';
import { AppBar, Toolbar, Typography, Button, Box } from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';

export const Header: React.FC = () => {
    return (
        <AppBar position="static">
            <Toolbar>
                <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
                    Todo List
                </Typography>
                <Box>
                    <Button color="inherit" component={RouterLink} to="/">
                        Главная
                    </Button>
                    <Button color="inherit" component={RouterLink} to="/login">
                        Войти
                    </Button>
                    <Button color="inherit" component={RouterLink} to="/register">
                        Зарегистрироваться
                    </Button>
                </Box>
            </Toolbar>
        </AppBar>
    );
};
