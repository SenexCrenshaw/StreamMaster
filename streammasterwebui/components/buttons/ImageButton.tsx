import BaseButton from './BaseButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const ImageButton: React.FC<ChildButtonProperties> = ({ disabled = true, iconFilled = true, onClick, tooltip = '' }) => (
  <BaseButton disabled={disabled} icon="pi-image" iconFilled={iconFilled} onClick={onClick} tooltip={tooltip} />
);

export default ImageButton;
