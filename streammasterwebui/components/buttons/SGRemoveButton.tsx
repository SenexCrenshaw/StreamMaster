import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const SGRemoveButton: React.FC<ChildButtonProperties> = ({ onClick, tooltip }) => {
  return (
    <SMButton
      iconFilled={false}
      icon="pi-check-circle"
      onClick={onClick}
      severity="secondary"
      tooltip={tooltip}
      rounded
      style={{
        backgroundColor: 'green'
      }}
    />
  );
};

export default SGRemoveButton;
