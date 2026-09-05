import { useState } from 'react';
import { TextField } from '@mui/material';
import { withAuthForm } from '../hocs/withAuthForm';
import type { WithAuthFormProps } from '../hocs/withAuthForm';
import { register } from '../store/slices/authSlice';

const RegisterInner = ({ FormShell }: WithAuthFormProps) => {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  return (
    <FormShell
      title="Зарегистрироваться"
      submitLabel="Зарегистрироваться"
      onSubmit={(dispatch) =>
        dispatch(register({ email, firstName, lastName }))
      }
    >
      <TextField
        label="Имя"
        value={firstName}
        onChange={(e) => setFirstName(e.target.value)}
        required
      />
      <TextField
        label="Фамилия"
        value={lastName}
        onChange={(e) => setLastName(e.target.value)}
        required
      />
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

export const Register = withAuthForm(RegisterInner);
