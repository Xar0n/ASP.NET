import { Typography, Button } from '@mui/material';
import { Link } from 'react-router-dom';

export const NotFound = () => (
    <>
        <Typography variant="h4">404 — страница не найдена</Typography>
        <Button component={Link} to="/">На главную</Button>
    </>
);