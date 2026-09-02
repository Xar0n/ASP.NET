import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Box, TextField, Button, Typography } from '@mui/material';
import { useAppDispatch } from '../store/hooks';
import { register } from '../store/slices/authSlice';

export const Register = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const handleSubmit = (e: React.SubmitEvent<HTMLFormElement>) => {
    e.preventDefault();
    dispatch(register({ email, firstName, lastName }));
    navigate('/');
  };

  return (
    <Box component="form" onSubmit={handleSubmit} sx={{ display: 'flex', flexDirection: 'column', gap: 2, maxWidth: 400 }}>
      <Typography variant="h4">Зарегистрироваться</Typography>
      <TextField label="Имя" value={firstName} onChange={(e) => setFirstName(e.target.value)} required />
      <TextField label="Фамилия" value={lastName} onChange={(e) => setLastName(e.target.value)} required />
      <TextField label="Email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
      <TextField label="Пароль" type="password" value={password} onChange={(e) => setPassword(e.target.value)} required />
      <Button variant="contained" type="submit">Зарегистрироваться</Button>
    </Box>
  );
};