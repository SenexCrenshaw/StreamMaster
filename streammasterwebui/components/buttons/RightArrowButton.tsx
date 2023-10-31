import BaseButton, { type ChildButtonProperties } from './BaseButton';

const RightArrowButton: React.FC<ChildButtonProperties> = ({ disabled = false, onClick, tooltip = 'Add' }) => (
  <BaseButton disabled={disabled} icon="pi-chevron-right" iconFilled={false} onClick={onClick} severity="success" tooltip={tooltip} />
);

export default RightArrowButton;
