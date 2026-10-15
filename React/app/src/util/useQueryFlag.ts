import { useCallback } from 'react';
import { useSearchParams } from 'react-router-dom';

// Egyszer használatos URL-jelző (pl. /customer-orders?new=1 a műszerfal gyorsgombjaiból): az oldal megnézi,
// hogy be van-e állítva, és a consume-mal eltávolítja, hogy visszalépéskor / frissítéskor ne nyíljon meg újra.
export function useQueryFlag(name: string): [boolean, () => void] {
  const [params, setParams] = useSearchParams();
  const flagged = params.get(name) === '1';

  const consume = useCallback(() => {
    setParams((prev) => {
      const next = new URLSearchParams(prev);
      next.delete(name);
      return next;
    }, { replace: true });
  }, [name, setParams]);

  return [flagged, consume];
}
