import BaseButton from './BaseButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const AutoSetButton: React.FC<ChildButtonProperties> = ({ disabled = true, onClick, tooltip = 'Auto Set' }) => (
  <BaseButton disabled={disabled} icon="pi-sort-numeric-up-alt" onClick={onClick} tooltip={tooltip} />
);

export default AutoSetButton;
