import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const SGRemoveButton: React.FC<ChildButtonProperties> = ({ onClick, tooltip }) => {
  return <SMButton className="icon-green-filled" iconFilled={true} icon="pi-check-circle" onClick={onClick} severity="secondary" tooltip={tooltip} rounded />;
};

export default SGRemoveButton;
