import BaseButton from './BaseButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const SGAddButton: React.FC<ChildButtonProperties> = ({ onClick, tooltip }) => {
  return <BaseButton iconFilled={false} icon="pi-circle" onClick={onClick} severity="secondary" tooltip={tooltip} />;
};

export default SGAddButton;
