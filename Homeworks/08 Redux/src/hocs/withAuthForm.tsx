import type { ComponentType, FC, ReactNode } from 'react';
import { Box, Button, Typography } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch } from '../store/hooks';
import type { AppDispatch } from '../store/store';

export interface AuthFormProps {
  title: string;
  submitLabel: string;
  onSubmit: (dispatch: AppDispatch) => void;
  children: ReactNode;
}

export interface WithAuthFormProps {
  FormShell: FC<AuthFormProps>;
}

export function withAuthForm<P extends object>(
  Wrapped: ComponentType<P & WithAuthFormProps>,
) {
  return function AuthFormWrapper(props: P) {
    const dispatch = useAppDispatch();
    const navigate = useNavigate();

    const FormShell: FC<AuthFormProps> = ({
      title,
      submitLabel,
      onSubmit,
      children,
    }) => (
      <Box
        component="form"
        onSubmit={(e) => {
          e.preventDefault();
          onSubmit(dispatch);
          navigate('/');
        }}
        sx={{ display: 'flex', flexDirection: 'column', gap: 2, maxWidth: 400 }}
      >
        <Typography variant="h4">{title}</Typography>
        {children}
        <Button variant="contained" type="submit">
          {submitLabel}
        </Button>
      </Box>
    );

    return <Wrapped {...props} FormShell={FormShell} />;
  };
}
