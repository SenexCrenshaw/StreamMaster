import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const SGAddButton: React.FC<ChildButtonProperties> = ({ onClick, tooltip }) => {
  return <SMButton iconFilled={false} icon="pi-circle" onClick={onClick} severity="secondary" tooltip={tooltip} />;
};

export default SGAddButton;
