import { InputNumber } from 'primereact/inputnumber';
import { useState } from 'react';

const TestPanel = () => {
  const [input, setInput] = useState<number>(1);
  return (
    <div className="col-1">
      <InputNumber className="border-1" value={input} onChange={(e) => setInput(e.value ?? 0)} />;
    </div>
  );
};
export default TestPanel;
