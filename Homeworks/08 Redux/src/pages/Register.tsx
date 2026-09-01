import { Box, TextField, Button, Typography } from '@mui/material';

export const Register = () => (
  <Box component="form" sx={{ display: 'flex', flexDirection: 'column', gap: 2, maxWidth: 400 }}>
    <Typography variant="h4">Зарегистрироваться</Typography>
    <TextField label="Email" type="email" />
    <TextField label="Пароль" type="password" />
    <Button variant="contained" type="submit">Зарегистрироваться</Button>
  </Box>
);