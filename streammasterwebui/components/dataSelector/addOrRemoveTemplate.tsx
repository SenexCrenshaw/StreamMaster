import AddButton from '@components/buttons/AddButton';
import MinusButton from '@components/buttons/MinusButton';

export function addOrRemoveTemplate(data: object) {
  const isSelected = true;
  if (!isSelected) {
    return (
      <div className="flex justify-content-between align-items-center p-0 m-0">
        <MinusButton iconFilled={false} onClick={() => console.log('AddButton', data)} />
      </div>
    );
  }
  return (
    <div className="flex justify-content-between align-items-center p-0 m-0">
      <AddButton iconFilled={false} onClick={() => console.log('AddButton', data)} />
    </div>
  );
}
