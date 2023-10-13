import BaseButton, { type ChildButtonProps } from './BaseButton';

const ImageButton: React.FC<ChildButtonProps> = ({ disabled = true, iconFilled = true, onClick, tooltip = '' }) => {
  return <BaseButton disabled={disabled} icon="pi-image" iconFilled={iconFilled} onClick={onClick} tooltip={tooltip} />;
};

export default ImageButton;
