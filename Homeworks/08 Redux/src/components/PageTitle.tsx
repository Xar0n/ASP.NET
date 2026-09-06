import React from 'react';
import { Typography, type TypographyProps } from '@mui/material';

type PageTitleProps = {
  title: string;
  variant?: TypographyProps['variant']; 
};

export const PageTitle: React.FC<PageTitleProps> = ({ title, variant = 'h4' }) => {
  return (
    <Typography variant={variant}>
      {title}
    </Typography>
  );
};
