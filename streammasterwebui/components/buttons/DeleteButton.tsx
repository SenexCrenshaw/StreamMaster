import BaseButton, { type ChildButtonProperties } from './BaseButton';

const DeleteButton: React.FC<ChildButtonProperties> = ({ disabled = false, iconFilled, label, onClick, tooltip = 'Delete Stream' }) => (
  <BaseButton disabled={disabled} icon="pi-minus" iconFilled={false} label={label} onClick={onClick} severity="danger" tooltip={tooltip} />
);

export default DeleteButton;
