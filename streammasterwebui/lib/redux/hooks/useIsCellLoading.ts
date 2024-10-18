import { Logger } from '@lib/common/logger';
import { useAppDispatch, useAppSelector } from '../hooks';
import { RootState } from '../store';
import { selectIsCellLoading, selectIsRowLoading, setCellLoading } from './loading';
import { useMemo, useCallback } from 'react';

interface UseIsCellLoadingProps {
  Entity: string;
  Id: string;
  Field: string;
}

const useIsCellLoading = ({ Entity, Id, Field }: UseIsCellLoadingProps): [boolean, (isLoading: boolean) => void] => {
  const dispatch = useAppDispatch();

  // Memoize selector results to avoid unnecessary recalculations
  const isRowLoading = useAppSelector(useMemo(() => (state: RootState) => selectIsRowLoading(state, Entity, Id), [Entity, Id]));

  const isCellLoadingInt = useAppSelector(useMemo(() => (state: RootState) => selectIsCellLoading(state, Entity, Id, Field), [Entity, Id, Field]));

  const isCellLoading = useMemo(() => isRowLoading || isCellLoadingInt, [isRowLoading, isCellLoadingInt]);

  const setIsCellLoading = useCallback(
    (isLoading: boolean) => {
      Logger.debug(`Setting cell loading for ${Entity} ${Id} ${Field} to ${isLoading}`);
      dispatch(setCellLoading({ entity: Entity, field: Field, id: Id, isLoading }));
    },
    [Entity, Field, Id, dispatch]
  );

  return [isCellLoading, setIsCellLoading];
};

export default useIsCellLoading;
