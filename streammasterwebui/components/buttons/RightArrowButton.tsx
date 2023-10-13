import { type ChildButtonProps } from './BaseButton';
import BaseButton from './BaseButton';

const RightArrowButton: React.FC<ChildButtonProps> = ({ disabled = false, onClick, tooltip = 'Add' }) => {
  return <BaseButton disabled={disabled} icon="pi-chevron-right" iconFilled={false} onClick={onClick} severity="success" tooltip={tooltip} />;
};

export default RightArrowButton;
