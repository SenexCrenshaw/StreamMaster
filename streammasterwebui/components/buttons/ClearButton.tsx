import BaseButton, { type ChildButtonProperties } from './BaseButton';

const ClearButton: React.FC<ChildButtonProperties> = ({ disabled = true, onClick, tooltip = '' }) => (
  <BaseButton disabled={disabled} icon="pi-book" onClick={onClick} tooltip={tooltip} />
);

export default ClearButton;
