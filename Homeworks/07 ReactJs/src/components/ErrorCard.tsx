interface ErrorCardProps {
    error: string;
}

function ErrorCard({ error }: ErrorCardProps) {
    return (
        <div style={{ backgroundColor: 'red', color: 'white', padding: '10px', borderRadius: '5px', margin: '10px 0' }}>
            <h2>Ошибка</h2>
            <p>{error}</p>
        </div>
    )
}

export default ErrorCard;