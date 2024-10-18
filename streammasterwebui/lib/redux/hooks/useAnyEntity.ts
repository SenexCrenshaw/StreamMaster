import { useCallback } from 'react';
import { useAppDispatch, useAppSelector } from '../hooks'; // Import the hook provided by your Redux store setup
import { removeAnyValue, setAnyValue } from '../slices/anySlice';

function useAnyEntity(key: string) {
  const dispatch = useAppDispatch();

  const setEntityValue = useCallback(
    (value: any) => {
      dispatch(setAnyValue({ key, value }));
    },
    [dispatch, key]
  );

  const removeEntityValue = useCallback(() => {
    dispatch(removeAnyValue(key));
  }, [dispatch, key]);

  const data = useAppSelector((state) => state.any);

  return { setEntityValue, removeEntityValue, data };
}

export default useAnyEntity;
