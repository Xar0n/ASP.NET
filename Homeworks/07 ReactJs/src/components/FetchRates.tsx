import { useState } from "react";
import RatesCard from "./RatesCard";
import type { Currency } from "../types";
import ErrorCard from "./ErrorCard";


interface DailyRates {
    Date: string
    Valute: Record<string, Currency>
}


function FetchRates() {
    const [rates, setRates] = useState<Currency[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [date, setDate] = useState<string | null>(null);

    async function handlerClick()
    {
        setLoading(true);
        setError(null);
        setRates([]);
        setDate(null);

        try {
            const response = await fetch("https://www.cbr-xml-daily.ru/daily_json.js");

            if (!response.ok) {
                setError(`Ошибка HTTP: ${response.status}`);
                return;
            }

            const data = await response.json() as DailyRates;
            setDate(data.Date);
            setRates(Object.values(data.Valute));
        }
        catch (error) {
            setError("Не удалось загрузить курсы валют");
        }
        finally {
            setLoading(false);
        }
    }
    

    return (
        <div>
            <h1>Курсы валют</h1>
            <button onClick={handlerClick}>Загрузить курсы</button>
            {loading && <p>Загрузка...</p>}
            {error && <ErrorCard error={error} />}
            {rates.length > 0 && date && <RatesCard date={date} rates={rates} />}
        </div>
    )
}

export default FetchRates;