import BaseButton, { type ChildButtonProps as ChildButtonProperties } from './BaseButton';

const ImageButton: React.FC<ChildButtonProperties> = ({ disabled = true, iconFilled = true, onClick, tooltip = '' }) => (
  <BaseButton disabled={disabled} icon="pi-image" iconFilled={iconFilled} onClick={onClick} tooltip={tooltip} />
);

export default ImageButton;
