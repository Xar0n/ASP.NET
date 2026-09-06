import { useState } from 'react';
import { withAuthForm } from '../hocs/withAuthForm';
import type { WithAuthFormProps } from '../hocs/withAuthForm';
import { login } from '../store/slices/authSlice';
import { TextFieldInput } from '../components/TextField';
import { Typography } from '@mui/material';

const LoginInner = ({ AuthFormShell }: WithAuthFormProps) => {
  const [email, setEmail] = useState<string>('');
  const [password, setPassword] = useState<string>('');
  const [error, setError] = useState<string>('');
  
  return (
    <AuthFormShell
      title="Войти"
      submitLabel="Войти"
      onSubmit={(dispatch) => {
        dispatch(login({ email }));
        setError('Неверный email или пароль');
      }}
    >
      <TextFieldInput
        label="Email"
        type="email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        required
      />
      
      <TextFieldInput
        label="Пароль" type="password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        required
      />
      {error && <Typography color="error">{error}</Typography>}
    </AuthFormShell>
  );
};

export const Login = withAuthForm(LoginInner);
