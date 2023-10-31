import { type ChildButtonProps as ChildButtonProperties } from './BaseButton';
import BaseButton from './BaseButton';

const RightArrowButton: React.FC<ChildButtonProperties> = ({ disabled = false, onClick, tooltip = 'Add' }) => (
  <BaseButton disabled={disabled} icon="pi-chevron-right" iconFilled={false} onClick={onClick} severity="success" tooltip={tooltip} />
);

export default RightArrowButton;
