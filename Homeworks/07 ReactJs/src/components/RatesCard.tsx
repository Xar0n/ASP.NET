import type { Currency } from "../types";

interface RatesCardProps {
    date: string;
    rates: Currency[];
}

function RatesCard({ date, rates }: RatesCardProps) {
    return (
        <div>
            <h2>Курсы валют на {date}</h2>
            <ul>
                {rates.map((rate) => (
                    <li key={rate.CharCode}>{rate.CharCode} - {rate.Value}</li>
                ))}
            </ul>
        </div>
    )
}

export default RatesCard;