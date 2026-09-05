import { TextField } from "@mui/material";

export const TextFieldInput = ({ label, value, onChange, required, type }: 
    { label: string, value: string, type: 'text' | 'password' | 'email', onChange: (e: React.ChangeEvent<HTMLInputElement>) => void, required: boolean }) =>
{
    return (
        <TextField
            label={label}
            value={value}
            type={type}
            onChange={onChange}
            required={required}
        />
    );
}