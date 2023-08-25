;

import { type ChildButtonProps } from "./BaseButton";
import BaseButton from "./BaseButton";

const BookButton: React.FC<ChildButtonProps> = ({ disabled = true, onClick, tooltip = "" }) => {
  return (
    <BaseButton
      disabled={disabled}
      icon="pi-book"
      onClick={onClick}
      tooltip={tooltip}
    />
  );
};

export default BookButton;
