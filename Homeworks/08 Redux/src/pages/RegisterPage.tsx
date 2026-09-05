import { useState } from 'react';
import { withAuthForm } from '../hocs/withAuthForm';
import type { WithAuthFormProps } from '../hocs/withAuthForm';
import { register } from '../store/slices/authSlice';
import { TextFieldInput } from '../components/TextField';

const RegisterInner = ({ AuthFormShell }: WithAuthFormProps) => {
  const [firstName, setFirstName] = useState<string>('');
  const [lastName, setLastName] = useState<string>('');
  const [email, setEmail] = useState<string>('');
  const [password, setPassword] = useState<string>('');

  return (
    <AuthFormShell
      title="Зарегистрироваться"
      submitLabel="Зарегистрироваться"
      onSubmit={(dispatch) =>
        dispatch(register({ email, firstName, lastName }))
      }
    >
      <TextFieldInput
        label="Имя"
        type="text"
        value={firstName}
        onChange={(e) => setFirstName(e.target.value)}
        required
      />
      <TextFieldInput
        label="Фамилия"
        type="text"
        value={lastName}
        onChange={(e) => setLastName(e.target.value)}
        required
      />
      <TextFieldInput
        label="Email"
        type="email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        required
      />
      <TextFieldInput
        label="Пароль"
        type="password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        required
      />
    </AuthFormShell>
  );
};

export const Register = withAuthForm(RegisterInner);
