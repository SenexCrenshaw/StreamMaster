import SMButton from '@components/sm/SMButton';
import { useIsTrue } from '@lib/redux/hooks/isTrue';

interface SMSimpleButtonProperties {
  dataKey: string;
}

const SimpleButton = ({ dataKey }: SMSimpleButtonProperties) => {
  const { isTrue, setIsTrue } = useIsTrue(dataKey);

  return (
    <SMButton
      icon="pi-window-maximize"
      iconFilled
      buttonClassName="sm-simple-button icon-blue"
      onClick={() => {
        setIsTrue(!isTrue);
      }}
      tooltip={isTrue ? 'Expand' : 'Shrink'}
    />
  );
};

export default SimpleButton;
