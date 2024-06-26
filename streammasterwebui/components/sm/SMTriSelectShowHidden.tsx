import SMButton from '@components/sm/SMButton';
import { useShowHidden } from '@lib/redux/hooks/showHidden';

import { useCallback, useMemo } from 'react';

interface SMTriSelectShowHiddenProperties {
  readonly dataKey: string;
}

export const SMTriSelectShowHidden = ({ dataKey }: SMTriSelectShowHiddenProperties) => {
  const { showHidden, setShowHidden } = useShowHidden(dataKey);

  const getToolTip = useMemo((): string => {
    if (showHidden === null) {
      return 'All';
    }

    if (showHidden === true) {
      return 'Visible';
    }

    return 'Hidden';
  }, [showHidden]);

  const moveNext = useCallback(() => {
    if (showHidden === null) {
      setShowHidden(true);
      return;
    }

    if (showHidden === true) {
      setShowHidden(false);
      return;
    }

    setShowHidden(null);
  }, [setShowHidden, showHidden]);

  const getIcon = useMemo(() => {
    if (showHidden === null) {
      return 'pi-eye';
    }

    if (showHidden === true) {
      return 'pi-eye';
    }

    return 'pi-eye-slash';
  }, [showHidden]);

  const getColor = useMemo(() => {
    if (showHidden === null) {
      return 'icon-yellow';
    }

    if (showHidden === true) {
      return 'icon-green';
    }

    return 'icon-red';
  }, [showHidden]);

  return <SMButton icon={getIcon} iconFilled={false} buttonClassName={getColor} onClick={() => moveNext()} rounded tooltip={getToolTip} />;
};
