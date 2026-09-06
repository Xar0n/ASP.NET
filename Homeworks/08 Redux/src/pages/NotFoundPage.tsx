import { Button } from '@mui/material';
import { Link } from 'react-router-dom';
import { PageTitle } from '../components';

export const NotFound = () => (
    <>
        <PageTitle title="404 — страница не найдена" />
        <Button component={Link} to="/">На главную</Button>
    </>
);