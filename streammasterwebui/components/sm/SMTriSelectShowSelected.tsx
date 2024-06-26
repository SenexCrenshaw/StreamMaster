import SMButton from '@components/sm/SMButton';
import { useShowSelected } from '@lib/redux/hooks/showSelected';

import { useCallback, useMemo } from 'react';

interface SMTriSelectShowHiddenProperties {
  readonly dataKey: string;
}

export const SMTriSelectShowSelected = ({ dataKey }: SMTriSelectShowHiddenProperties) => {
  const { showSelected, setShowSelected } = useShowSelected(dataKey);

  const getToolTip = useMemo((): string => {
    if (showSelected === null) {
      return 'All';
    }

    if (showSelected === true) {
      return 'Selected';
    }

    return 'Not Selected';
  }, [showSelected]);

  const moveNext = useCallback(() => {
    if (showSelected === null) {
      setShowSelected(true);
      return;
    }

    if (showSelected === true) {
      setShowSelected(false);
      return;
    }

    setShowSelected(null);
  }, [setShowSelected, showSelected]);

  const getIcon = useMemo(() => {
    if (showSelected === null) {
      return 'pi-eye';
    }

    if (showSelected === true) {
      return 'pi-eye';
    }

    return 'pi-eye-slash';
  }, [showSelected]);

  const getColor = useMemo(() => {
    if (showSelected === null) {
      return 'icon-yellow';
    }

    if (showSelected === true) {
      return 'icon-green';
    }

    return 'icon-red';
  }, [showSelected]);

  return <SMButton icon={getIcon} iconFilled={false} buttonClassName={getColor} onClick={() => moveNext()} rounded tooltip={getToolTip} />;
};
