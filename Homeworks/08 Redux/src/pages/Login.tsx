import { useState } from 'react';
import { TextField } from '@mui/material';
import { withAuthForm } from '../hocs/withAuthForm';
import type { WithAuthFormProps } from '../hocs/withAuthForm';
import { login } from '../store/slices/authSlice';

const LoginInner = ({ FormShell }: WithAuthFormProps) => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  return (
    <FormShell
      title="Войти"
      submitLabel="Войти"
      onSubmit={(dispatch) => dispatch(login({ email }))}
    >
      <TextField
        label="Email"
        type="email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        required
      />
      <TextField
        label="Пароль"
        type="password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        required
      />
    </FormShell>
  );
};

export const Login = withAuthForm(LoginInner);
