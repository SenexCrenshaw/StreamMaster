import { type ChildButtonProps } from "./BaseButton";
import BaseButton from "./BaseButton";


const XButton: React.FC<ChildButtonProps> = ({ onClick, tooltip = 'Remove' }) => {
  return (
    <BaseButton
      icon="pi-times"
      iconFilled={false}
      isLeft
      onClick={onClick}
      rounded
      severity="danger"
      tooltip={tooltip}
    />
  );
};

export default XButton;
