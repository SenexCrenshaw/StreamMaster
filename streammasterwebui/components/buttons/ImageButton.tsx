import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const ImageButton: React.FC<ChildButtonProperties> = ({ disabled = true, iconFilled = true, onClick, tooltip = '' }) => (
  <SMButton disabled={disabled} icon="pi-image" iconFilled={iconFilled} onClick={onClick} tooltip={tooltip} />
);

export default ImageButton;
